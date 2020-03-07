import { Component, OnInit } from '@angular/core';
import { MyAngularUiService } from './my-angular-ui/my-angular-ui.service';


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    title = 'MyAngularUiTest';

    constructor(private mau: MyAngularUiService) {}

    ngOnInit(): void {
        this.mau.Start(3000);
    }
}
