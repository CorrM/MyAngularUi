import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { MauUtils } from './mau-utils';
import { EventEmitter } from '@angular/core';
import { MauComponent } from './mau.service';

export enum RequestType {
    None = 0,
    SetEvents = 1,
    EventCallback = 2,
    GetPropValue = 3,
    SetPropValue = 4,
    ExecuteCode = 5,
    SetVarValue = 6,
    CallMethod = 7,
    ReceiveMethod = 8,
    SetStyle = 9,
    RemoveStyle = 10,
    AddClass = 11,
    RemoveClass = 12,
    DotNetReady = 13,
    CustomData = 14
}

export interface MauRequestInfo {
    RequestId: number,
    MauComponent: MauComponent,
    RequestType: RequestType,
    Data: any
}

export interface WebSocketOnMessageCallback {
    (message: any): void;
}

export class MyAngularUiWebSocket {

    private _url: string;
    private _reconnectTime: number;
    private _wsSubject: WebSocketSubject<any>;
    private _thisArg: any;

    public OnMessageCB: WebSocketOnMessageCallback;

    public IsRunning: boolean = false;
    public IsConnected: boolean = false;

    // Events
    public OnConnect: EventEmitter<void>;
    public OnDisConnect: EventEmitter<void>;

    constructor(thisArg: any) {
        this._thisArg = thisArg;

        this.OnConnect = new EventEmitter();
        this.OnDisConnect = new EventEmitter();
    }

    public Start(url: string, reconnectTime: number): boolean {
        if (this.OnMessageCB === undefined) {
            throw "'OnMessageCB' must assign";
        }
        if (this.IsRunning) {
            return false;
        }
        this.IsRunning = true;

        this._url = url;
        this._reconnectTime = reconnectTime;

        this.ReConnect();
        return true;
    }

    public Stop(): void {
        this._wsSubject = null;
        this.IsConnected = false;
        this.IsRunning = false;
    }

    private ReConnect(): void {
        this._wsSubject = webSocket({
            url: this._url,
            openObserver: {
                // OnConnect
                next: (val) => {
                    this.IsConnected = true;
                    this.OnConnect.emit();
                }
            }
        });
        this._wsSubject.subscribe(
            msg => this.OnMessageRecv(msg),
            err => this.OnError(err),
            () => this.OnClose()
        );
    }

    /**
     * Only use when you want to send response to .Net request,
     *
     * @param {number} requestId .Net request id
     * @param {string} mauComponentId MauId of your target
     * @param {RequestType} requestType Type of request
     * @param {*} data Request data
     * @returns {boolean} Send state
     * @memberof WebSocketManager
     */
    public Send(requestId: number, mauComponent: MauComponent, requestType: RequestType, data: any): boolean {
        // Check
        if (!this.IsConnected) {
            console.log(`SendData Can't send to closed socket.`);
            return false;
        }
        if (!this._wsSubject) {
            return false;
        }

        // Send
        this._wsSubject.next({
            requestId: requestId,
            requestType: requestType,
            mauComponentId: mauComponent?.Id ?? "",
            data: !data ? {} : data
        });
        return true;
    }

    /**
     * Use to send new request to .Net side
     *
     * @param {string} mauComponent Your target
     * @param {RequestType} requestType Type of request
     * @param {*} data Request data
     * @returns {boolean} Send state
     * @memberof WebSocketManager
     */
    public SendRequest(mauComponent: MauComponent, requestType: RequestType, data: any): boolean {
        let requestId: number = MauUtils.GetRandomInt(1, 100000);
        return this.Send(requestId, mauComponent, requestType, data);
    }

    public SendEventCallback(mauComponent: MauComponent, eventName: string, eventType: string, data: any): boolean {
        return this.SendRequest(mauComponent, RequestType.EventCallback, { eventName: eventName, eventType: eventType, data: data });
    }

    private OnMessageRecv(msg: any): void {
        // Call `Callback function`
        this.OnMessageCB.call(this._thisArg, msg);
    }

    private OnError(err: any): void {
        if (this.IsConnected) {
            this.OnDisConnect.emit();
        }
        this.IsConnected = false;

        console.log(`Error: Reconnecting after ${this._reconnectTime} ms.`);
        setTimeout(() => this.ReConnect(), this._reconnectTime);
    }

    private OnClose(): void {
        if (this.IsConnected) {
            this.OnDisConnect.emit();
        }
        this.IsConnected = false;

        console.log(`Closed: Reconnecting after ${this._reconnectTime} ms.`);
        setTimeout(() => this.ReConnect(), this._reconnectTime);
    }
}
