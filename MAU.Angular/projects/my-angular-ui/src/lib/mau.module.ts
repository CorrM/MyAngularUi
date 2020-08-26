import { NgModule } from '@angular/core';
import { MauElementDirective } from './mau-element.directive';

const MauTypes = [
    MauElementDirective
];

@NgModule({
    declarations: MauTypes,
    imports: [],
    exports: MauTypes
})
export class MyAngularUiModule { }
