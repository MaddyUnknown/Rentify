import { Observable, Subject } from 'rxjs';

export class ObservableMap<K, V> extends Map<K, V> {
  private valueChageObserver$: Subject<void> = new Subject<void>();

  public override set(key: K, value: V): this {
    super.set(key, value);
    this.valueChageObserver$.next();
    return this;
  }

  public override delete(key: K): boolean {
    const result = super.delete(key);
    this.valueChageObserver$.next();
    return result;
  }

  public override clear(): void {
    super.clear();
    this.valueChageObserver$.next();
  }

  public get valueChange(): Observable<void> {
    return this.valueChageObserver$.asObservable();
  }
}
