import { Directive, ElementRef, Input, OnInit, ViewContainerRef, OnDestroy } from '@angular/core';
import { MyAngularUiService, AppInjector, MauComponentProp, MauComponentEvent } from './mau.service';

@Directive({
    selector: `[mauId]`
})
export class MauElementDirective implements OnInit, OnDestroy {
    private mau: MyAngularUiService;

    @Input("mauId")
    public ElementId: string;

    private _viewRef: any;

    constructor(private _viewContainerRef: ViewContainerRef, private el: ElementRef) {
        this._viewRef = this.GetComponent();
        this.mau = AppInjector.get(MyAngularUiService);
    }

    public GetComponent(): any {
        const container = this._viewContainerRef["_lContainer"][0];
        if (container instanceof HTMLElement) {
            return container;
        }
        return container[8];
    }

    public ngOnInit(): void {
        this.mau.SetElement({
            Id: this.ElementId,
            Native: this.el,
            Component: this._viewRef,
            HandledEvents: new Map<string, MauComponentEvent>(),
            HandledProps: new Map<string, MauComponentProp>()
        });
    }

    public ngOnDestroy(): void {
        this.mau.GetElement(this.ElementId).HandledProps.forEach((prop: MauComponentProp) => {
            prop.Listen = false;
        });

        this.mau.GetElement(this.ElementId).HandledEvents.forEach((event: MauComponentEvent) => {
            if (event.Unsubscribe) {
                event.Unsubscribe();
            }
            event.Handled = false;
        });

        this.mau.GetElement(this.ElementId).Component = null;
        this.mau.GetElement(this.ElementId).Native = null;
    }
}
