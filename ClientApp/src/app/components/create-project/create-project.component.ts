import { Component, EventEmitter, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { Project } from '../../models/project.model';

@Component({
  selector: 'app-create-project',
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.css']
})
export class CreateProjectComponent {
  @Output() projectCreated = new EventEmitter<void>(); // Notify parent when project is created
  createProjectForm!: FormGroup;
  isSubmitting = false;
  errorMessage = '';

  constructor(private fb: FormBuilder, private projectService: ProjectService) { }

  ngOnInit(): void {
    this.createProjectForm = this.fb.group({
      // Project Details
      projectDetails: this.fb.group({
        projectName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        startDate: ['', Validators.required],
        estimatedCompletionDate: ['', Validators.required],
        budget: [0, [Validators.required, Validators.min(1)]],
        notes: this.fb.array([this.createNote()]) 
      }),

      // Client Information
      client: this.fb.group({
        clientName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        address: this.fb.group({
          street: ['', Validators.required],
          city: ['', Validators.required],
          state: ['', Validators.required],
          zipCode: ['', [Validators.required, Validators.pattern('^[0-9]{5}$')]]
        }),
        // Dynamic Client Contacts
        clientContacts: this.fb.array([
          this.createClientContact() // At least one contact by default
        ]), 
      }),
    });

    console.log("Form initialized:", this.createProjectForm.value)
  }

  createClientContact(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10,15}$')]] // Allow 10-15 digit phone numbers
    });
  }

  // Getter for client contacts
  get clientContacts(): FormArray {
    return this.createProjectForm.get('client.clientContacts') as FormArray;
  }

  // Add new client contact field
  addClientContact(): void {
    this.clientContacts.push(this.createClientContact());
  }

  // Remove a client contact field
  removeClientContact(index: number): void {
    if (this.clientContacts.length > 1) {
      this.clientContacts.removeAt(index);
    }
  }

  get notes(): FormArray {
    return this.createProjectForm.get('projectDetails.notes') as FormArray;
  }

  createNote(): FormGroup {
    return this.fb.group({
      noteText: ['', [Validators.required, Validators.maxLength(500)]]
    });
  }

  addNote(): void {
    this.notes.push(this.createNote());
  }

  removeNote(index: number): void {
    if (this.notes.length > 1) {
      this.notes.removeAt(index);
    }
  }

  createProject(): void {
    if (this.createProjectForm.invalid) {
      alert('Please fill all required fields correctly.');
      return;
    }

    this.isSubmitting = true;
    const newProject: Project = this.createProjectForm.value;

    this.projectService.createProject(newProject).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.projectCreated.emit(); // Notify parent to refresh the project list
      },
      error: (err) => {
        this.isSubmitting = false;
        this.errorMessage = 'Failed to create project. Please try again.';
        console.error('Project creation error:', err);
      }
    });
  }
}
