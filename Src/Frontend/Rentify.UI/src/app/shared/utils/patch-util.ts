export function patchMapWithList<K, V1, V2>(
  map: Map<K, V1>,
  list: V2[],
  keySelector: (v: V2) => K,
  createNew: (v: V2) => V1,
  modifyExisting: (existing: V1, newValue: V2) => V1,
  deleteExisting: (v: V1) => void,
): void {
  const newIds = new Set(list.map(keySelector));

  //Remove
  for (const [key, value] of map) {
    if (!newIds.has(key)) {
      deleteExisting(value);
      map.delete(key);
    }
  }

  //Add/Modify
  for (const value of list) {
    const key = keySelector(value);
    const existingValue = map.get(key);
    if (existingValue) {
      map.set(key, modifyExisting(existingValue, value));
    } else {
      map.set(key, createNew(value));
    }
  }
}
