import { NgModule, Injector } from '@angular/core';
import { UiElementDirective } from './ui-element.directive';

export let AppInjector: Injector;

@NgModule({
    declarations: [UiElementDirective],
    imports: [
    ],
    exports: [UiElementDirective]
})
export class MyAngularUiModule {
    constructor(private injector: Injector) {
        AppInjector = this.injector;
    }
}
