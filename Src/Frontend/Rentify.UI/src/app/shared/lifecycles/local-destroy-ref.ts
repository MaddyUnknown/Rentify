export class LocalDestroyRef {
  private callbacks: (() => void)[] = [];

  onDestroy(cb: () => void) {
    this.callbacks.push(cb);
  }

  destroy() {
    for (const cb of this.callbacks) cb();
    this.callbacks.length = 0;
  }

  static create() {
    return new LocalDestroyRef();
  }
}
