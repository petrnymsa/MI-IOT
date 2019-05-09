import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';


export default class NgbTimeStructHelper {
    static isGreater(first: NgbTimeStruct, second: NgbTimeStruct): boolean {

        if (first.hour > second.hour ||
            (
                first.hour === second.hour
                && first.minute > second.minute
            ) ||
            (first.hour === second.hour
                && first.minute === second.minute
                && first.second > second.second)) {
            return true;
        }

        return false;
    }
}