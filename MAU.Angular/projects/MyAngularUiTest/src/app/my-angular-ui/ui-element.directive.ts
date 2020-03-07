import { Directive, ElementRef, Input, OnInit, HostListener } from '@angular/core';
import { MyAngularUiService, AppInjector } from './my-angular-ui.service';

@Directive({
    selector: '[mauUiElement]'
})
export class UiElementDirective implements OnInit {
    private UiService: MyAngularUiService;

    @Input("mauUiElement")
    ElementId: string;

    constructor(private el: ElementRef) {
        this.UiService = AppInjector.get(MyAngularUiService);
    }

    ngOnInit() {
        this.UiService.AddElement(this.ElementId, this.el);
    }

    @HostListener("*")
    OnEvent(event: Event) {
        console.log(event);
    }
}
