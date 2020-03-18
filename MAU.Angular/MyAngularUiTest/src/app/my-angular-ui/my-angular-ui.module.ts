import { NgModule, Injector } from '@angular/core';
import { UiElementDirective } from './ui-element.directive';


@NgModule({
    declarations: [UiElementDirective],
    imports: [
    ],
    exports: [UiElementDirective]
})
export class MyAngularUiModule { }
