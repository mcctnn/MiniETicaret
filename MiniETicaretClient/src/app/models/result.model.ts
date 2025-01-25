export class ResultModel<T>{
    data?:T |null;
    errorMessages?:string[]|null;
    isSuccesful:boolean=false;
}