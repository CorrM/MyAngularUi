import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'lib-MyAngularUi',
  template: `
    <p>
      my-angular-ui works!
    </p>
  `,
  styles: []
})
export class MyAngularUiComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
