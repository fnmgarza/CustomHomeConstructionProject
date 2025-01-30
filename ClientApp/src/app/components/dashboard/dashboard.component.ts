import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { Project, ProjectStatus } from '../../models/project.model';
import { debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  originalProjects: Project[] = []
  projects: Project[] = [];
  projectStatuses: ProjectStatus[] = [];
  projectsGrouped: { [key: string]: any[] } = {};
  searchForm!: FormGroup;
  selectedProject: any = null;
  isEmpty = true;
  creatingProject = false;
  collapsedStatuses: { [key: string]: boolean } = {}; 

  constructor(private fb: FormBuilder, private projectService: ProjectService) {}

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      searchQuery: ['']
    });

    this.searchForm.get('searchQuery')!.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => {
      this.filterProjects(query);
    });

    this.loadProjects();
    this.loadStatuses();
  }

  toggleStatus(statusName: string): void {
    this.collapsedStatuses[statusName] = !this.collapsedStatuses[statusName];
  }

  filterProjects(query: string): void {
    if (!query) {
      this.projects = [...this.originalProjects];
    } else {
      query = query.toLowerCase();
      this.projects = this.originalProjects.filter(p =>
        p.projectDetails.projectName.toLowerCase().includes(query) ||
        p.client.clientName.toLowerCase().includes(query)
      );
    }
    this.groupProjectsByStatus();
  }

  loadStatuses(): void {
    this.projectService.getProjectStatuses().subscribe({
      next: (statuses) => {
        console.log("All statuses from API:", statuses);
        this.projectStatuses = statuses;
        this.groupProjectsByStatus();
      },
      error: (err) => {
        console.error("Error loading statuses:", err);
      }
    });
  }

  loadProjects(): void {
    this.projectService.getProjects().subscribe({
      next: (data: Project[]) => {
        console.log("✅ Projects received from API:", data); // Log entire project list

        this.originalProjects = data;
        this.projects = [...this.originalProjects];
        this.isEmpty = this.projects.length === 0;
        this.groupProjectsByStatus();
      },
      error: (err) => {
        console.error("Error loading projects:", err);
      }
    });
  }

  getUniqueStatuses(projects: Project[]): ProjectStatus[] {
    const uniqueStatusesMap = new Map<string, ProjectStatus>();

    projects.forEach(p => {
      if (!uniqueStatusesMap.has(p.projectDetails.status.statusName)) {
        uniqueStatusesMap.set(p.projectDetails.status.statusName, {
          statusName: p.projectDetails.status.statusName,
          color: p.projectDetails.status.color
        });
      }
    });

    return Array.from(uniqueStatusesMap.values());
  }

  groupProjectsByStatus(): void {
    this.projectsGrouped = this.projectStatuses.reduce((acc, status) => {
      acc[status.statusName] = this.projects.filter(
        p => p.projectDetails.status.statusName === status.statusName
      );
      return acc;
    }, {} as { [key: string]: any[] });

    this.projectStatuses.forEach(status => {
      if (!this.projectsGrouped[status.statusName]) {
        this.projectsGrouped[status.statusName] = [];
      }
    });
  }

  selectProject(project: any): void {
    this.selectedProject = project;
  }

  saveProject(updatedProject: any): void {
    const index = this.projects.findIndex(p => p.projectDetails.projectName === updatedProject.projectDetails.projectName);
    if (index > -1) {
      this.projects[index] = updatedProject;
    }
    this.selectedProject = null;
    this.groupProjectsByStatus();
  }

  onProjectUpdated(updatedProject: Project): void {
    if (!updatedProject || !updatedProject.projectDetails) {
      console.error("Error: Received a null or undefined updatedProject.");
      return;
    }
    const index = this.projects.findIndex(p => p.projectDetails.id === updatedProject.projectDetails.id);
    if (index > -1) {
      this.projects[index] = updatedProject;
      this.groupProjectsByStatus();
    }
  }

  onProjectDeleted(projectId: number): void {
    this.projects = this.projects.filter(p => p.projectDetails.id !== projectId);
    this.groupProjectsByStatus();
  }

  showCreateProject(): void {
    this.creatingProject = true;
  }

  onProjectCreated(): void {
    this.creatingProject = false;
    this.loadProjects(); // Refresh the list
  }
}

