// toast.service.ts
import { Injectable, signal } from '@angular/core';
import { Toast, ToastType } from '../model/toast.model';

@Injectable({ providedIn: 'root' })
export class ToastService {

  private counter = 0;

  toasts = signal<Toast[]>([]);

  show(message: string, type: ToastType = 'info', duration = 3000) {
    const id = ++this.counter;

    const toast: Toast = { id, message, type, duration };

    this.toasts.update(t => [...t, toast]);

    setTimeout(() => this.remove(id), duration);
  }

  remove(id: number) {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }

  success(message: string, duration?: number) {
    this.show(message, 'success', duration);
  }

  error(message: string, duration?: number) {
    this.show(message, 'error', duration);
  }

  info(message: string, duration?: number) {
    this.show(message, 'info', duration);
  }

  warning(message: string, duration?: number) {
    this.show(message, 'warning', duration);
  }
}