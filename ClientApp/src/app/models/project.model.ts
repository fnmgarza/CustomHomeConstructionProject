export interface Project {
  projectDetails: ProjectDetails;
  client: Client;
}

export interface ProjectDetails {
  id?: number;
  projectName: string;
  startDate: string;
  estimatedCompletionDate: string;
  budget: number;
  status: ProjectStatus;
  notes?: ProjectNote[];
  photoUrl?: string;
}

export interface ProjectNote {
  id?: number;
  noteText: string;
}
export interface ProjectStatus {
  id?: number
  statusName: string;
  color: string;
}

export interface Client {
  id?: number
  clientName: string;
  address: Address;
  clientContacts: ClientContact[];
}

export interface Address {
  id?: number
  street: string;
  city: string;
  state: string;
  zipCode: string;
}

export interface ClientContact {
  id?: number;
  name: string;
  email: string;
  phone: string;
}
