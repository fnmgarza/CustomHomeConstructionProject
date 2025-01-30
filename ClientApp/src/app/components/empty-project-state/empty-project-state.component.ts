import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-empty-project-state',
  templateUrl: './empty-project-state.component.html',
  styleUrls: ['./empty-project-state.component.css']
})
export class EmptyProjectStateComponent {
  @Output() createProject = new EventEmitter<void>(); // Event to notify parent

  onCreateProject(): void {
    this.createProject.emit(); // Emit event when button is clicked
  }
}
