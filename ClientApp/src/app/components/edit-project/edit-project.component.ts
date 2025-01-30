import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Project, ProjectStatus } from '../../models/project.model';
import { ProjectService } from '../../services/project.service';

@Component({
  selector: 'app-edit-project',
  templateUrl: './edit-project.component.html',
  styleUrls: ['./edit-project.component.css']
})
export class EditProjectComponent {
  @Input() project!: Project;
  @Output() projectUpdated = new EventEmitter<Project>();
  isOpen: boolean = false;
  editProjectForm!: FormGroup;
  errorMessage!: string;
  statuses: ProjectStatus[] = [];
  constructor(private fb: FormBuilder, private projectService: ProjectService) { }

  ngOnInit(): void {
    this.isOpen = false;
    this.initializeForm();
    this.loadProjectStatuses();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['project'] && this.project) {
      if (!this.editProjectForm) {
        this.initializeForm();
      }
      this.patchForm();
    }
  }

  loadProjectStatuses(): void {
    this.projectService.getProjectStatuses().subscribe({
      next: (statuses) => {
        this.statuses = statuses;
      },
      error: (err) => console.error('Error fetching project statuses:', err)
    });
  }

  openModal() {
    this.isOpen = true;
    if (this.project) {
      this.patchForm();
    }
  }

  closeModal() {
    this.isOpen = false;
  }

  initializeForm(): void {
    this.editProjectForm = this.fb.group({
      projectDetails: this.fb.group({
        projectName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        startDate: ['', Validators.required],
        statusId: [null, Validators.required],
        estimatedCompletionDate: ['', Validators.required],
        budget: ['', [Validators.required, Validators.min(1)]],
        notes: this.fb.array([this.createNote()]) // FormArray for notes
      }),
      client: this.fb.group({
        clientName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        address: this.fb.group({
          street: ['', Validators.required],
          city: ['', Validators.required],
          state: ['', Validators.required],
          zipCode: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(5)]]
        }),
        clientContacts: this.fb.array([this.createClientContact()]) // FormArray for contacts
      })
    });

    if (this.project) {
      this.patchForm();
    }
  }

  patchForm(): void {
    if (!this.project) return;

    const selectedStatusId = this.project.projectDetails?.status?.id || null;

    this.editProjectForm.patchValue({
      projectDetails: {
        projectName: this.project.projectDetails?.projectName || '',
        startDate: this.formatDate(this.project.projectDetails?.startDate) || '',
        estimatedCompletionDate: this.formatDate(this.project.projectDetails?.estimatedCompletionDate) || '',
        budget: this.project.projectDetails?.budget || '',
        statusId: selectedStatusId,
      },
      client: {
        clientName: this.project.client?.clientName || '',
        address: {
          street: this.project.client?.address?.street || '',
          city: this.project.client?.address?.city || '',
          state: this.project.client?.address?.state || '',
          zipCode: this.project.client?.address?.zipCode || ''
        },
      }
    });

    // Update Notes FormArray
    this.updateNotesFormArray();

    // Update Client Contacts FormArray
    this.updateClientContactsFormArray();
  }

  updateNotesFormArray() {
    const notesArray = this.notes;
    notesArray.clear();
    const projectNotes = this.project?.projectDetails?.notes || [];
    if (projectNotes.length > 0) {
      projectNotes.forEach(note => {
        notesArray.push(this.fb.control(note.noteText || ''));
      });
    } else {
      notesArray.push(this.fb.control(''));
    }
  }

  updateClientContactsFormArray() {
    const contactsArray = this.clientContacts;
    contactsArray.clear();

    const clientContacts = this.project?.client?.clientContacts || [];

    if (clientContacts.length === 0) {
      contactsArray.push(this.createClientContact());
    } else {
      clientContacts.forEach(contact => {
        contactsArray.push(this.fb.group({
          name: [contact.name || '', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
          email: [contact.email || '', [Validators.required, Validators.email]],
          phone: [contact.phone || '', [Validators.required, Validators.minLength(10), Validators.maxLength(15)]]
        }));
      });
    }

    console.log("Client Contacts Updated:", contactsArray.value);
  }


  formatDate(date: string | Date | null | undefined): string {
    if (!date) return '';
    return new Date(date).toISOString().split('T')[0];
  }

  createClientContact(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10,15}$')]]
    });
  }
  get clientContacts(): FormArray {
    return this.editProjectForm?.get('client.clientContacts') as FormArray || this.fb.array([]);
  }
  get notes(): FormArray {
    return this.editProjectForm?.get('projectDetails.notes') as FormArray || this.fb.array([]);
  }

  createNote(): FormGroup {
    return this.fb.group({
      noteText: ['', [Validators.required, Validators.maxLength(500)]]
    });
  }

  addNote() {
    this.notes.push(this.fb.control(''));
  }

  removeNote(index: number) {
    if (this.notes.length > 1) {
      this.notes.removeAt(index);
    }
  }

  addClientContact() {
    this.clientContacts.push(this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(15)]]
    }));
  }

  removeClientContact(index: number) {
    this.clientContacts.removeAt(index);
  }

  saveProject(): void {
    if (!this.project || !this.editProjectForm.valid) {
      this.errorMessage = "Please fill out all fields."
      return
    } 

    const existingNotes = this.project.projectDetails?.notes || [];

    const updatedProject: Project = {
      ...this.project,
      projectDetails: {
        ...this.project.projectDetails,
        ...this.editProjectForm.value.projectDetails,
        status: this.statuses.find(status => status.id === this.editProjectForm.value.projectDetails.statusId), 

        notes: this.notes.controls.map((noteControl, index) => ({
          id: existingNotes[index]?.id || null,
          noteText: noteControl.value
        }))
      },
      client: {
        ...this.project.client,
        ...this.editProjectForm.value.client
      }
    };

    delete (updatedProject.projectDetails as any).statusId;

    this.projectService.updateProject(updatedProject).subscribe({
      next: (response) => {
        if (!response || !response.projectDetails) {
          console.error("Error: API returned an invalid response.", response);
          return;
        }
        this.projectUpdated.emit(response);
        this.closeModal();
      },
      error: (err) => {
        console.error("Error updating project:", err);
      }
    });
  }
}
