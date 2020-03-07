import { Injectable, ElementRef, Injector } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { UtilsService } from './utils.service';
import { Subject } from 'rxjs';

export let AppInjector: Injector;

enum RequestType {
    None = 0,
    GetEvents = 1,
    EventCallback = 2,
}

@Injectable({
    providedIn: 'root'
})
export class MyAngularUiService {
    private _connected: boolean = false;
    private _reconnectTime: number = 2;
    private _eventsTimerId;
    private _subject: WebSocketSubject<any>;
    private _orders: Map<number, any>;

    public UiElements: Map<string, ElementRef>;
    public UiElementEvents: Map<string, string[]>;
    public Port: number;

    constructor(private utils: UtilsService, private injector: Injector) {
        this.UiElements = new Map<string, ElementRef>();
        this.UiElementEvents = new Map<string, string[]>();
        this._orders = new Map<number, any>();

        AppInjector = this.injector;
    }

    public Start(port: number): void {
        if (this._subject) {
            return;
        }

        console.log("Connecting: Try to connect");
        this.Port = port;
        this.Connect();

        // Get handled events
        // this._eventsTimerId = setInterval(() => {
        //     this.UiElementEvents.forEach((value: string[], key: string) => {
        //         if (value.length == 0) {
        //             this.UiElementEvents.set(key, this.GetEvents(key));
        //         }
        //     });
        // }, 3000);
    }

    public Stop(): void {
        clearInterval(this._eventsTimerId);
    }

    private Connect(): void {
        this._subject = webSocket({
            url: `ws://localhost:${this.Port}/UiHandler`,
            openObserver: { next: val => { this._connected = true; console.log("opened"); } }
        });
        this._subject.subscribe(
            msg => () => this.OnMessage(msg),
            err => () => this.OnError(err),
            () => this.OnClose()
        );
    }

    public AddElement(uiElementId: string, el: ElementRef) {
        this.UiElements.set(uiElementId, el);
        this.UiElementEvents.set(uiElementId, []);
    }

    public SendData(uiElementId: string, requestType: RequestType, data: any): number {
        console.log(`SendData Called.`);

        if (this._subject.closed) {
            console.log(`SendData Can't send to closed socket.`);
            return -1;
        }

        // Check
        if (!this._subject) {
            return -1;
        }
        if (!this.UiElements.has(uiElementId)) {
            return -1;
        }

        // Get unique orderid
        let orderId: number = this.utils.GetRandomInt(5000);
        while (this._orders.has(orderId)) {
            orderId = this.utils.GetRandomInt(5000);
        }

        console.log(`Send new order ${orderId}`);

        // Send
        this._subject.next({
            orderId: orderId,
            requestType: requestType,
            uiElementId: uiElementId,
            data: !data ? {} : data
        });
        return orderId;
    }

    public SendEventCallback(uiElementId: string, eventName: string, data: any): number {
        return this.SendData(uiElementId, RequestType.EventCallback, { eventName: eventName, data: data });
    }

    private GetEvents(uiElementId: string): string[] {
        // Send request to get handled events in .NET
        let orderId: number = this.SendData(uiElementId, RequestType.GetEvents, {});

        if (orderId == -1) {
            return null;
        }

        // Get response
        let data = this.GetMessage(orderId);

        if (!data) {
            return [];
        }
        console.log(`Data Recevied ${data}`);
        return data["events"];
    }

    private GetMessage(orderId: number): any {
        if (this._orders.has(orderId)) {
            let retData = this._orders.get(orderId);
            this._orders.delete(orderId);

            return retData;
        }

        return null;
    }

    private OnMessage(msg: any): void {
        let orderId: number = msg["orderId"];

        if (this._orders.has(orderId)) {
            this._orders.set(orderId, msg);
        } else {
            console.log(`Unhandled order ${orderId} !!`)
        }

        console.log(`Recived a new order ${orderId}`);
    }

    private OnError(err: any): void {
        this._connected = false;
        console.log(`Error: Reconnecting after ${this._reconnectTime} sec.`);
        setTimeout(() => this.Connect(), this._reconnectTime * 1000);
    }

    private OnClose(): void {
        this._connected = false;
        console.log(`Closed: Reconnecting after ${this._reconnectTime} sec.`);
        setTimeout(() => this.Connect(), this._reconnectTime * 1000);
    }
}
