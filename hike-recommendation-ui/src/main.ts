import { bootstrapApplication } from '@angular/platform-browser';
// Update the import path below to the correct location of HikeResultComponent
import { HikeResultComponent } from './app/pages/hike-result/hike-result.component';

bootstrapApplication(HikeResultComponent).catch(err => console.log(err));
