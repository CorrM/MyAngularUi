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
}
