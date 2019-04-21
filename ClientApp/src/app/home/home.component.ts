import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public foo: String = "FooBar";

  public changeFoo() {

    this.foo = "ChangedFoo";
  }
}
