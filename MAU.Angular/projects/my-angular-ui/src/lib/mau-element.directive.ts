import { Directive, ElementRef, Input, OnInit, ViewContainerRef, OnDestroy } from '@angular/core';
import { MyAngularUiService, AppInjector, MauComponentProp } from './mau.service';

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
            HandledEvents: new Map<string, boolean>(),
            HandledProps: new Map<string, MauComponentProp>()
        });
    }

    public ngOnDestroy(): void {
        this.mau.GetElement(this.ElementId).HandledProps.forEach((prop: MauComponentProp, propName: string) => {
            prop.Listen = false;
        });
    }
}
