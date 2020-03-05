import { Injectable, ElementRef } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';


@Injectable({
    providedIn: 'root'
})
export class MyAngularUiService {
    private subject: WebSocketSubject<unknown>;

    public UiElements: Map<string, ElementRef>;
    public Port: number;

    constructor() {
        this.UiElements = new Map<string, ElementRef>();
    }

    public Start(port: number): void {
        if (this.subject) {
            return;
        }

        console.log("Connecting: Try to connect");
        this.Port = port;
        this.Connect();
    }

    private Connect(): void {
        this.subject = webSocket(`ws://localhost:${this.Port}/UiHandler`);
        this.subject.subscribe(
            msg => this.OnMessage(msg),
            err => this.OnError(err),
            () => this.OnClose()
        );
    }

    public SendEvent(uiElementId: string, eventName: string, data: any): boolean {
        if (this.subject) {
            return false;
        }
        if (!this.UiElements.has(uiElementId)) {
            return false;
        }

        this.subject.next({ uiElementId: uiElementId, eventName: eventName, data: data });
        return true;
    }

    public OnMessage(msg: any): void {
        console.log('message received: ' + msg["message"])
    }

    public OnError(err: any): void {
        console.log("Error: Reconnecting after 5 sec." + err);
        setTimeout(() => this.Connect(), 5000);
    }

    public OnClose(): void {
        console.log("Error: Reconnecting after 5 sec.");
        setTimeout(() => this.Connect(), 5000);
    }
}
