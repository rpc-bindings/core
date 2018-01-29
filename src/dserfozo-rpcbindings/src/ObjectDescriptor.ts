export interface MethodDescriptor {
    id: number;
    name: string;
}

export interface ObjectDescriptor {
    id: number;
    name: string;
	methods:Map<number, MethodDescriptor>;
}