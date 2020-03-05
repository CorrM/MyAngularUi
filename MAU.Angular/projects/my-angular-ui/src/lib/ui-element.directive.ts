import { Directive, ElementRef, HostListener, Input, OnInit } from '@angular/core';
import { MyAngularUiService } from './my-angular-ui.service';
import { AppInjector } from './my-angular-ui.module';

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
        this.UiService.UiElements.set(this.ElementId, this.el);
    }

    @HostListener('click')
    onMouseEnter() {
        console.log(this.UiService);
    }
}
