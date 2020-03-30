import { Injectable, ElementRef, Injector, EventEmitter, Renderer2, RendererFactory2 } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { UtilsService } from './utils.service';

export let AppInjector: Injector;

interface MauElement {
    El: ElementRef,
    Component: any
}

enum RequestType {
    None = 0,
    GetEvents = 1,
    EventCallback = 2,
    GetPropValue = 3,
    SetPropValue = 4,
    ExecuteCode = 5,
    SetVarValue = 6
}

@Injectable({
    providedIn: 'root'
})
export class MyAngularUiService {
    private static reconnectTime: number = 1;
    private _connected: boolean = false;
    private _subject: WebSocketSubject<any>;
    private _variables: Map<string, any>;
    private _renderer: Renderer2;

    public Mutation: MutationObserver;
    public UiElements: Map<string, MauElement>;
    public UiElementEvents: Map<string, string[]>;
    public Port: number;

    constructor(
        private utils: UtilsService,
        private injector: Injector,
        private rendererFactory: RendererFactory2) {
        this._renderer = rendererFactory.createRenderer(null, null);
        this._variables = new Map<string, any>();

        this.UiElements = new Map<string, MauElement>();
        this.UiElementEvents = new Map<string, string[]>();

        AppInjector = this.injector;
        this.Mutation = new MutationObserver((mutations: MutationRecord[]) => {
            mutations.forEach((mutation: MutationRecord) => {
                if (mutation.type == "attributes") {
                    const uiElementId: string = (<HTMLElement>mutation.target).getAttribute("mauuielement");
                    const attribute: string = mutation.attributeName;

                    // Send data to .Net
                    this.GetProp(uiElementId, true, attribute);
                }
                else if (mutation.type == "childList") {
                    const uiElementId: string = (<HTMLElement>mutation.target).getAttribute("mauuielement");

                    // Send data to .Net
                    this.GetProp(uiElementId, false, "innerHTML");
                    this.GetProp(uiElementId, false, "innerText");
                    this.GetProp(uiElementId, false, "textContent");
                }
                else if (mutation.type == "characterData") {
                    let htmlEl: HTMLElement = mutation.target.parentElement;

                    // Get our UiElement Maximum 15Lvl Depth
                    for (let index = 0; index < 15; index++) {
                        if (htmlEl.hasAttribute("mauuielement")) {
                            const uiElementId: string = htmlEl.getAttribute("mauuielement");

                            // Send data to .Net
                            this.GetProp(uiElementId, false, "innerHTML");
                            this.GetProp(uiElementId, false, "innerText");
                            this.GetProp(uiElementId, false, "textContent");
                            return;
                        }
                        else {
                            htmlEl = htmlEl.parentElement;
                        }
                    }
                }
            });
        });
    }

    public Start(port: number): void {
        if (this._subject) {
            return;
        }

        console.log("Connecting: Try to connect");
        this.Port = port;
        this.Connect();
        this.InitElements();
    }

    public Stop(): void {
        this.Mutation.disconnect();
    }

    private Connect(): void {
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

    private OnMessage(msg: any): void {
        console.log(msg);

        // Handle request
        let requestType: RequestType = msg["requestType"];
        let uiElementId: string = msg["uiElementId"];
        let data: any = msg["data"];

        switch (requestType) {
            case RequestType.GetEvents:
                // Set events
                this.UiElementEvents.set(uiElementId, data["events"]);

                // Set event handler
                let events = this.UiElementEvents.get(uiElementId);
                events.forEach((eventName: string) => {
                    let mauEl: MauElement = this.UiElements.get(uiElementId);

                    // Access event as property
                    let compEvent: EventEmitter<any> = mauEl.Component[eventName];

                    // * Set subscribe to EventEmitter, that's for some custom components
                    // * Like Angular Matiral components (MatSelect, ...)
                    if (compEvent !== undefined) {
                        compEvent.subscribe({
                            next: (event: MessageEvent) => {
                                this.FireEvent(uiElementId, eventName, event);
                            }
                        });
                    }
                    // * Add normal listener if it's normal HTML Element
                    else {
                        // ToDo: Add checker to `listen`, so it's not duplicate event handler
                        this._renderer.listen(mauEl.El.nativeElement, eventName, (event: Event) => {
                            this.FireEvent(uiElementId, undefined, event);
                        });
                    }
                });
                break;

            case RequestType.GetPropValue:
                this.GetProp(uiElementId, data["propIsAttr"], data["propName"]);
                break;

            case RequestType.SetPropValue:
                this.SetProp(uiElementId, data["propIsAttr"], data["propName"], data["propVal"]);
                break;

            case RequestType.ExecuteCode:
                this.ExecuteCode(uiElementId, data["code"]);
                break;

            case RequestType.SetVarValue:
                this.SetVar(data["varName"], data["varValue"]);
                break;

            default:
                break;
        }
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

    private Send(uiElementId: string, requestType: RequestType, data: any): boolean {
        // Check
        if (!this._connected) {
            console.log(`SendData Can't send to closed socket.`);
            return false;
        }
        if (!this._subject || !this.UiElements.has(uiElementId)) {
            return false;
        }

        // Send
        this._subject.next({
            requestType: requestType,
            uiElementId: uiElementId,
            data: !data ? {} : data
        });
        return true;
    }

    private SendEventCallback(uiElementId: string, eventName: string, eventType: string, data: any): boolean {
        return this.Send(uiElementId, RequestType.EventCallback, { eventName: eventName, eventType: eventType, data: data });
    }

    public AddElement(uiElementId: string, el: MauElement) {
        this.UiElements.set(uiElementId, el);
        this.UiElementEvents.set(uiElementId, []);
    }

    private InitElements(): void {
        setInterval(() => {
            // Events
            this.UiElementEvents.forEach((events: string[], uiElementId: string) => {
                if (events.length == 0 && this._connected) {
                    this.GetEvents(uiElementId);
                }
            });
        }, 3000)
    }

    private GetEvents(uiElementId: string): void {
        this.Send(uiElementId, RequestType.GetEvents, {});
    }

    private FireEvent(uiElementId: string, eventName: string, event: Event): void {
        let eName: string = event.type ? event.type : eventName;
        this.SendEventCallback(uiElementId, eName, event.constructor.name, this.utils.ObjectToJson(event));
    }

    private GetProp(uiElementId: string, propIsAttr: boolean, propName: string): void {
        if (!this.UiElements.has(uiElementId)) {
            return;
        }

        let val: any;
        if (propIsAttr) {
            val = this.UiElements.get(uiElementId).El.nativeElement.getAttribute(propName);
        }
        else {
            val = this.UiElements.get(uiElementId).El.nativeElement[propName];
        }

        this.Send(uiElementId, RequestType.GetPropValue, { propName: propName, propValue: val });
    }

    private SetProp(uiElementId: string, propIsAttr: boolean, propName: string, propVal: string): void {

        if (propIsAttr) {
            this.UiElements.get(uiElementId).El.nativeElement.setAttribute(propName, propVal);
        }
        else {
            this.UiElements.get(uiElementId).El.nativeElement[propName] = propVal;
        }
    }

    private ExecuteCode(uiElementId: string, code: string) {
        // Access the element (So in TS code just use 'uiElementId' to access the mauElement)
        let elSelector: string = `let elCorrM = this.UiElements.get(uiElementId).El.nativeElement;`;
        code = code.replace(uiElementId, "elCorrM");
        code = `${elSelector}\n${code}`;

        const codeExecuter = {
            Run: (pString: string) => {
                return eval(pString)
            }
        };

        codeExecuter.Run.call(this, code);
    }

    private SetVar(varName: string, varVal: any) {
        this._variables.set(varName, varVal);
    }

    public GetVar(component: any, varName: string): any {
        let fullVarName = `${component.constructor.name}_${varName}`;

        return this._variables.has(fullVarName)
            ? this._variables.get(fullVarName)
            : null;
    }

}
