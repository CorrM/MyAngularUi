import { Directive, ElementRef, Input, OnInit, HostListener } from '@angular/core';
import { MyAngularUiService, AppInjector } from './my-angular-ui.service';

@Directive({
    selector: '[mauUiElement]'
})
export class UiElementDirective implements OnInit {
    private uiService: MyAngularUiService;

    @Input("mauUiElement")
    ElementId: string;

    constructor(private el: ElementRef) {
        this.uiService = AppInjector.get(MyAngularUiService);
    }

    ngOnInit() {
        this.uiService.AddElement(this.ElementId, this.el);

        this.uiService.Mutation.observe(this.el.nativeElement, { characterData: true, attributes: true, childList: true, subtree: true });
    }
}
