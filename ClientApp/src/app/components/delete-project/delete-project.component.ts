import { Component, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { ProjectService } from '../../services/project.service';

@Component({
  selector: 'app-delete-project',
  templateUrl: './delete-project.component.html',
  styleUrls: ['./delete-project.component.css']
})
export class DeleteProjectComponent {
  @Input() projectName!: string;
  @Input() projectId!: number;
  @Output() projectDeleted = new EventEmitter<number>();

  isOpen: boolean = false; // Ensure it's false on load

  constructor(private projectService: ProjectService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.isOpen = false;
  }
  openModal() {
    this.isOpen = true;
    this.cdr.detectChanges();
  }

  closeModal() {
    this.isOpen = false;
    this.cdr.detectChanges();
  }

  confirmDelete(): void {
    if (!this.projectId) return;

    this.projectService.deleteProject(this.projectId).subscribe({
      next: () => {
        this.projectDeleted.emit(this.projectId);
        this.closeModal();
      },
      error: (err) => console.error('Error deleting project:', err)
    });
  }
}

