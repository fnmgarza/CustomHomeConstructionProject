import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project, ProjectStatus } from '../models/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = './api/projects';

  constructor(private http: HttpClient) { }

  getProjectStatusCounts(): Observable<any> {
    return this.http.get(`${this.apiUrl}/status-counts`);
  }

  getProjectStatuses(): Observable<ProjectStatus[]> {
    return this.http.get<ProjectStatus[]>(`${this.apiUrl}/statuses`);
  }

  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.apiUrl}/get-projects`);
  }

  searchProjects(query: string): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.apiUrl}/search?query=${query}`);
  }

  updateProject(project: Project): Observable<Project> {
    return this.http.put<Project>(`${this.apiUrl}/edit/${project.projectDetails.id}`, project);
  }

  deleteProject(projectId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete/${projectId}`);
  }

  createProject(project: Project): Observable<Project> {
    return this.http.post<Project>(`${this.apiUrl}/create-project`, project);
  }
}
