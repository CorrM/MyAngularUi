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
    private subject: WebSocketSubject<unknown>;
    private orders: Map<number, any>;

    public UiElements: Map<string, ElementRef>;
    public Port: number;

    constructor(private utils: UtilsService) {
        this.UiElements = new Map<string, ElementRef>();
        this.orders = new Map<number, any>();
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

    public SendData(uiElementId: string, requestType: RequestType, data: any): number {
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
    }

    public SendEventCallback(uiElementId: string, eventName: string, data: any): number {
        return this.SendData(uiElementId, RequestType.EventCallback, { eventName: eventName, data: data });
    }

    public async GetEvents(uiElementId: string) {
        // Send request to get handled events in .NET
        let orderId: number = this.SendData(uiElementId, RequestType.GetEvents, {});

        // Get response
        let data = await this.GetMessage(orderId);

        console.log(`Data Recevied ${data}`);
    }

    private async GetMessage(orderId: number): Promise<any> {
        return new Promise<any>(async (resolve) => {
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
