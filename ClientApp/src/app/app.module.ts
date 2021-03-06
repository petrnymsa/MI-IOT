import { DateFormatPipe } from './util/DateFormatter';
import { Settings } from './data/Settings';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDatepickerModule,
  MatInputModule,
  MatNativeDateModule
} from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { ChartsModule } from 'ng2-charts';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RoomComponent } from './room/room.component';
import { RealTimeRoomComponent } from './real-time-room/real-time-room.component';
import { SettingsComponent } from './settings/settings.component';
import { FlowerComponent } from './flower/flower.component';
import { FlowerRealTimeComponent } from './flower-real-time/flower-real-time.component';
import { NgbModule, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateCustomParserFormatter } from './util/NgDateCustomParseFormatter';
import { StateComponent } from './state/state.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    RoomComponent,
    RealTimeRoomComponent,
    SettingsComponent,
    FlowerComponent,
    FlowerRealTimeComponent,
    StateComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'settings', component: SettingsComponent }
    ]),
    ChartsModule,
    MatDatepickerModule,
    MatInputModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    NgbModule
  ],
  providers: [DateFormatPipe, { provide: NgbDateParserFormatter, useClass: NgbDateCustomParserFormatter }],
  bootstrap: [AppComponent]
})
export class AppModule { }
