import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-icon-button',
  templateUrl: './icon-button.component.html',
  styleUrls: ['./icon-button.component.css']
})
export class AppIconButtonComponent {
  @Input() icon!: string; // Icon class (e.g., 'fa fa-trash')
  @Input() color!: string; // Button color
  @Input() tooltip: string = ''; // Optional tooltip
  @Input() type: string = 'button';
  @Output() clicked = new EventEmitter<void>(); // Click event emitter

  onClick(): void {
      this.clicked.emit();
  }
}
