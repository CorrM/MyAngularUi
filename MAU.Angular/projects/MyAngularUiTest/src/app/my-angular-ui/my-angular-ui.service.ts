import { Injectable, ElementRef } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { UtilsService } from './utils.service';

enum RequestType {
    None = 0,
    GetEvents = 1,
    EventCallback = 2,
}

@Injectable({
    providedIn: 'root'
})
export class MyAngularUiService {
    private reconnectTime: number = 2;
    private eventsTimerId;
    private subject: WebSocketSubject<unknown>;
    private orders: Map<number, any>;

    public UiElements: Map<string, ElementRef>;
    public UiElementEvents: Map<string, string[]>;
    public Port: number;

    constructor(private utils: UtilsService) {
        this.UiElements = new Map<string, ElementRef>();
        this.UiElementEvents = new Map<string, string[]>();
        this.orders = new Map<number, any>();
    }

    public Start(port: number): void {
        if (this.subject) {
            return;
        }

        console.log("Connecting: Try to connect");
        this.Port = port;
        this.Connect();

        // Get
        this.eventsTimerId = setInterval(() => {
            this.UiElementEvents.forEach(async (value: string[], key: string) => {
                if (!value) {
                    this.UiElementEvents.set(key, await this.GetEvents(key));
                }
            });
        }, 3000);
    }

    public Stop(): void {
        clearInterval(this.eventsTimerId);
    }

    private Connect(): void {
        this.subject = webSocket(`ws://localhost:${this.Port}/UiHandler`);
        this.subject.subscribe(
            msg => this.OnMessage(msg),
            err => this.OnError(err),
            () => this.OnClose()
        );
    }

    public async SendData(uiElementId: string, requestType: RequestType, data: any): Promise<number> {
        return new Promise<number>(() => {
            console.log(`SendData Called.`);

            // Check
            if (!this.subject) {
                return -1;
            }
            if (!this.UiElements.has(uiElementId)) {
                return -1;
            }

            // Get unique orderid
            let orderId: number = this.utils.GetRandomInt(5000);
            while (this.orders.has(orderId)) {
                orderId = this.utils.GetRandomInt(5000);
            }

            console.log(`Send new order ${orderId}`);

            // Send
            this.subject.next({
                orderId: orderId,
                requestType: requestType,
                uiElementId: uiElementId,
                data: !data ? {} : data
            });
            return orderId;
        });
    }

    public SendEventCallback(uiElementId: string, eventName: string, data: any): Promise<number> {
        return this.SendData(uiElementId, RequestType.EventCallback, { eventName: eventName, data: data });
    }

    private async GetEvents(uiElementId: string): Promise<string[]> {
        // Send request to get handled events in .NET
        let orderId: number = await this.SendData(uiElementId, RequestType.GetEvents, {});

        // Get response
        let data = await this.GetMessage(orderId);

        console.log(`Data Recevied ${data}`);

        return data["events"];
    }

    private GetMessage(orderId: number): Promise<any> {
        return new Promise<any>(async () => {
            while (this.orders) {
                if (this.orders.has(orderId)) {
                    let retData = this.orders.get(orderId);
                    this.orders.delete(orderId);

                    return retData;
                }
                await this.utils.Sleep(8);
            }

            return null;
        });
    }

    private OnMessage(msg: any): void {
        let orderId: number = msg["orderId"];

        if (this.orders.has(orderId)) {
            this.orders.set(orderId, msg);
        } else {
            console.log(`Unhandled order ${orderId} !!`)
        }

        console.log(`Recived a new order ${orderId}`);
    }

    private OnError(err: any): void {
        console.log(`Error: Reconnecting after ${this.reconnectTime} sec.`);
        setTimeout(() => this.Connect(), this.reconnectTime * 1000);
    }

    private OnClose(): void {
        console.log(`Closed: Reconnecting after ${this.reconnectTime} sec.`);
        setTimeout(() => this.Connect(), this.reconnectTime * 1000);
    }
}
