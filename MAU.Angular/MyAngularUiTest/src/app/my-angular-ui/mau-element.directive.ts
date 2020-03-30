import { Directive, ElementRef, Input, OnInit } from '@angular/core';
import { MyAngularUiService, AppInjector } from './my-angular-ui.service';
import { MatSelect } from '@angular/material/select';

@Directive({
    selector: '[mauId]'
})
export class MauElementDirective implements OnInit {
    private uiService: MyAngularUiService;

    @Input("mauId")
    ElementId: string;

    @Input("mauRef")
    ViewRef: any;

    constructor(private el: ElementRef) {
        this.uiService = AppInjector.get(MyAngularUiService);
    }

    ngOnInit() {
        this.uiService.AddElement(this.ElementId, { El: this.el, Component: this.ViewRef } );

        this.uiService.Mutation.observe(this.el.nativeElement, { characterData: true, attributes: true, childList: true, subtree: true });
    }
}
