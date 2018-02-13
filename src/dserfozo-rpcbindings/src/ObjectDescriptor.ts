export interface MethodDescriptor {
    id: number;
    name: string;
}

export interface PropertyDescriptor {
    id: number;
    name: string;
    writable: boolean;
    readable: boolean;
}

export interface ObjectDescriptor {
    id: number;
    name: string;
    methods: Map<number, MethodDescriptor> | any;
    properties: Map<number, PropertyDescriptor> | any;
}
