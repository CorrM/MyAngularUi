

export class MauUtils {
    public static readonly SelectorName: string = "mauid";

    public static GetRandomInt(min: number, max: number): number {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    public static async Sleep(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    public static IsNonEmptyString(str: string): boolean {
        return str && str.trim().length > 0;
    }

    // Thanks to => https://stackoverflow.com/a/58416333/3351489
    public static ObjectToJson(object: any, maxDepth: number = 3, depth: number = 0, firstCall: boolean = true): any {
        if (firstCall) {
            let objType = typeof (object);
            if (objType === "string" || objType === "boolean") {
                return JSON.stringify({ value: object });
            }
        }

        // change max_depth to see more levels
        if (depth > maxDepth)
            return 'Object';

        const obj = {};
        for (let key in object) {
            let value = object[key];
            if (value instanceof Node)
                // specify which properties you want to see from the node
                value = { id: (<any>value).id };
            else if (value instanceof Window)
                value = "Window";
            else if (value instanceof Object)
                value = this.ObjectToJson(value, maxDepth, depth + 1, false);

            obj[key] = value;
        }

        return depth ? obj : JSON.stringify(obj);
    }
}
