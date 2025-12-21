import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Subscription } from 'rxjs';
import { Toast, ToastService, ToastType } from '../../../core/services/toast.service';

interface ToastWithProgress extends Toast {
  progress: number;
}

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './toast.component.html',
})
export class ToastComponent implements OnInit, OnDestroy {
  toasts: ToastWithProgress[] = [];
  private subscription?: Subscription;
  private progressIntervals: Map<string, number> = new Map();

  constructor(private toastService: ToastService) {}

  ngOnInit(): void {
    this.subscription = this.toastService.toasts$.subscribe((toast) => {
      const toastWithProgress: ToastWithProgress = {
        ...toast,
        progress: 100,
      };
      this.toasts.push(toastWithProgress);
      this.startProgressAnimation(toastWithProgress);
      this.removeToastAfterDelay(toastWithProgress);
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.progressIntervals.forEach((interval) => clearInterval(interval));
    this.progressIntervals.clear();
  }

  removeToast(toast: ToastWithProgress): void {
    const interval = this.progressIntervals.get(toast.id);
    if (interval) {
      window.clearInterval(interval);
      this.progressIntervals.delete(toast.id);
    }
    this.toasts = this.toasts.filter((t) => t.id !== toast.id);
  }

  private startProgressAnimation(toast: ToastWithProgress): void {
    const duration = toast.duration || 3000;
    const interval = 16; // ~60fps
    const decrement = (100 / duration) * interval;

    const progressInterval = window.setInterval(() => {
      toast.progress = Math.max(0, toast.progress - decrement);
      if (toast.progress <= 0) {
        window.clearInterval(progressInterval);
        this.progressIntervals.delete(toast.id);
      }
    }, interval);

    this.progressIntervals.set(toast.id, progressInterval);
  }

  private removeToastAfterDelay(toast: ToastWithProgress): void {
    setTimeout(() => {
      this.removeToast(toast);
    }, toast.duration || 3000);
  }

  getToastIcon(type: ToastType): string {
    switch (type) {
      case 'success':
        return 'check_circle';
      case 'error':
        return 'error';
      case 'warning':
        return 'warning';
      case 'info':
      default:
        return 'info';
    }
  }

  getToastColorClass(type: ToastType): string {
    switch (type) {
      case 'success':
        return 'text-green-500';
      case 'error':
        return 'text-red-500';
      case 'warning':
        return 'text-yellow-500';
      case 'info':
      default:
        return 'text-blue-500';
    }
  }

  getToastBgClass(type: ToastType): string {
    switch (type) {
      case 'success':
        return 'bg-green-500/20 border-green-500/30';
      case 'error':
        return 'bg-red-500/20 border-red-500/30';
      case 'warning':
        return 'bg-yellow-500/20 border-yellow-500/30';
      case 'info':
      default:
        return 'bg-blue-500/20 border-blue-500/30';
    }
  }

  getProgressBarColorClass(type: ToastType): string {
    switch (type) {
      case 'success':
        return 'bg-green-500';
      case 'error':
        return 'bg-red-500';
      case 'warning':
        return 'bg-yellow-500';
      case 'info':
      default:
        return 'bg-blue-500';
    }
  }
}

