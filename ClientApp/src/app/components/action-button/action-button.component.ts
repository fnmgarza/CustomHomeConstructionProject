import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-action-button',
  templateUrl: './action-button.component.html',
  styleUrls: ['./action-button.component.css']
})
export class ActionButtonComponent {
  @Input() text?: string;
  @Input() type: string = 'button';
  @Input() color: 'primary' | 'danger' | 'success' | 'warning' | 'secondary' = 'primary';
  @Input() disabled: boolean = false;

  @Output() clicked = new EventEmitter<void>(); // Emit event when button is clicked

  onClick(): void {
    this.clicked.emit(); // Notify parent component
  }
}
