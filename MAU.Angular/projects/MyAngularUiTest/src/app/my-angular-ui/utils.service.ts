import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class UtilsService {

    constructor() { }

    public GetRandomInt(max: number) {
        return Math.floor(Math.random() * Math.floor(max));
    }

    public async Sleep(ms: number) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    // Thanks to => https://stackoverflow.com/a/58416333/3351489
    public ObjectToJson(object: any, depth = 0, max_depth = 2) {
        // change max_depth to see more levels, for a touch event, 2 is good
        if (depth > max_depth)
            return 'Object';

        const obj = {};
        for (let key in object) {
            let value = object[key];
            if (value instanceof Node)
                // specify which properties you want to see from the node
                value = { id: (<any>value).id };
            else if (value instanceof Window)
                value = 'Window';
            else if (value instanceof Object)
                value = this.ObjectToJson(value, depth + 1, max_depth);

            obj[key] = value;
        }

        return depth ? obj : JSON.stringify(obj);
    }
}
