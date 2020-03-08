import { Injectable, ElementRef, Injector } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { UtilsService } from './utils.service';
import { Subject } from 'rxjs';
import { EventManager } from '@angular/platform-browser';

export let AppInjector: Injector;

enum RequestType {
    None = 0,
    GetEvents = 1,
    EventCallback = 2,
    GetPropValue = 3,
    SetPropValue = 4
}

@Injectable({
    providedIn: 'root'
})
export class MyAngularUiService {
    private static reconnectTime: number = 2;
    private _connected: boolean = false;
    private _eventsTimerId;
    private _subject: WebSocketSubject<any>;
    private _orders: Map<number, any>;

    public UiElements: Map<string, ElementRef>;
    public UiElementEvents: Map<string, string[]>;
    public Port: number;

    constructor(private utils: UtilsService, private injector: Injector, private eventManager: EventManager) {
        this.UiElements = new Map<string, ElementRef>();
        this.UiElementEvents = new Map<string, string[]>();
        this._orders = new Map<number, any>();

        AppInjector = this.injector;
    }

    //#region WebSocket

    public Start(port: number): void {
        if (this._subject) {
            return;
        }

        console.log("Connecting: Try to connect");
        this.Port = port;
        this.Connect();

        // Get handled events
        this._eventsTimerId = setInterval(() => this.InitElements(), 3000);
    }

    public Stop(): void {
        clearInterval(this._eventsTimerId);
    }

    private Connect(): void {
        this._orders.clear();

        this._subject = webSocket({
            url: `ws://localhost:${this.Port}/UiHandler`,
            openObserver: { next: val => { this._connected = true; console.log("opened"); } }
        });
        this._subject.subscribe(
            msg => this.OnMessage(msg),
            err => this.OnError(err),
            () => this.OnClose()
        );
    }

    //#endregion

    private GetOrder(orderId: number): any {
        let retData: any = null;

        // while (this._orders && retData == null) {
        //     if (this._orders.has(orderId)) {
        //         retData = this._orders.get(orderId);
        //     }
        // }

        // this._orders.delete(orderId);
        return retData;
    }

    private OnMessage(msg: any): void {
        let orderId: number = msg["orderId"];

        // order mean it's will be handled in other place 
        if (this._orders.has(orderId)) {
            this._orders.set(orderId, msg);
            return;
        }

        // Handle request
        let requestType: RequestType = msg["requestType"];
        let uiElementId: string = msg["uiElementId"];
        let data: any = msg["data"];

        switch (requestType) {
            case RequestType.SetPropValue:
                this.SetProp(uiElementId, data["propName"], data["propVal"]);
                console.log(data);
                break;

            default:
                break;
        }

        console.log(`Recived a new order ${orderId}`);
    }

    private OnError(err: any): void {
        this._connected = false;
        console.log(`Error: Reconnecting after ${MyAngularUiService.reconnectTime} sec.`);
        setTimeout(() => this.Connect(), MyAngularUiService.reconnectTime * 1000);
    }

    private OnClose(): void {
        this._connected = false;
        console.log(`Closed: Reconnecting after ${MyAngularUiService.reconnectTime} sec.`);
        setTimeout(() => this.Connect(), MyAngularUiService.reconnectTime * 1000);
    }

    //#region Send

    private SendData(orderId: number, uiElementId: string, requestType: RequestType, data: any): number {
        // Check
        if (!this._connected) {
            console.log(`SendData Can't send to closed socket.`);
            return -1;
        }
        if (!this._subject) {
            return -1;
        }
        if (!this.UiElements.has(uiElementId)) {
            return -1;
        }

        this._orders.set(orderId, null);

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

    private Send(uiElementId: string, requestType: RequestType, data: any): number {
        // Get unique orderid
        let orderId: number = this.utils.GetRandomInt(5000);
        while (this._orders.has(orderId)) {
            orderId = this.utils.GetRandomInt(5000);
        }

        return this.SendData(orderId, uiElementId, requestType, data);
    }

    private SendEventCallback(uiElementId: string, eventName: string, data: any): number {
        return this.Send(uiElementId, RequestType.EventCallback, { eventName: eventName, data: data });
    }

    private SendAndRecv(uiElementId: string, requestType: RequestType, data: any): any {
        let orderId: number = this.Send(uiElementId, requestType, data);

        if (orderId == -1) {
            return null;
        }

        // Get response
        let dataRet = this.GetOrder(orderId);

        if (!dataRet) {
            return null;
        }

        return dataRet["data"];
    }

    //#endregion

    public AddElement(uiElementId: string, el: ElementRef) {
        this.UiElements.set(uiElementId, el);
        this.UiElementEvents.set(uiElementId, []);
    }

    private InitElements(): void {
        // Events
        this.UiElementEvents.forEach((events: string[], uiElementId: string) => {
            if (events.length == 0 && this._connected) {
                // Set events
                this.UiElementEvents.set(uiElementId, this.GetEvents(uiElementId));

                // Set event handler
                events = this.UiElementEvents.get(uiElementId);
                events.forEach((eventName: string) => {
                    let domObj = this.UiElements.get(uiElementId).nativeElement;
                    this.eventManager.addEventListener(domObj, eventName, (event: Event) => this.FireEvent(uiElementId, event));
                });
            }
        });
    }

    private GetEvents(uiElementId: string): string[] {
        let data = this.SendAndRecv(uiElementId, RequestType.GetEvents, {});

        if (!data) {
            return [];
        }

        return data["events"];
    }

    private FireEvent(uiElementId: string, event: Event): void {
        let eventName: string = event.type;
        this.SendEventCallback(uiElementId, eventName, {});
    }

    private GetProp(uiElementId: string, propName: string): void {
        let val = this.UiElements.get(uiElementId).nativeElement.getAttribute(propName);
        let data = this.SendAndRecv(uiElementId, RequestType.GetPropValue, { propName: propName, propValue: val });

        if (!data) {
            return;
        }

        return data["data"];
    }

    private SetProp(uiElementId: string, propName: string, propVal: string): void {
        this.UiElements.get(uiElementId).nativeElement.setAttribute(propName, propVal);
    }
}
