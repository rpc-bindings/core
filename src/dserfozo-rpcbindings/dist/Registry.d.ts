export declare class Registry<TSaved> {
    private readonly savedCallbacks;
    private lastId;
    save(callback: TSaved): number;
    get(id: number): TSaved;
    delete(id: number): void;
}
