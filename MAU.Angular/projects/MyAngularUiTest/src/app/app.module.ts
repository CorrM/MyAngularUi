import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { MyAngularUiModule } from 'my-angular-ui'

import { AppComponent } from './app.component';

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        MyAngularUiModule,
        BrowserModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
