import { HttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { FlexiGridModule } from 'flexi-grid';
import { TrCurrencyPipe } from 'tr-currency';
import { api } from '../../constants';
import { ResultModel } from '../../models/result.model';
import { ShoppingCartModel } from '../../models/shopping-cart.model';
import { OrderModel } from '../../models/order.model';

@Component({
  selector: 'app-orders',
  imports: [FlexiGridModule, TrCurrencyPipe],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent {
  orders=signal<OrderModel[]>([]);
  token=signal<string>("");

  constructor(
      private http: HttpClient,
    ) {
      if (localStorage.getItem("my-token")) {
        this.token.set(localStorage.getItem("my-token")!)
  
        this.getAll();
      }
    }
  
    getAll() {
      this.http.get<ResultModel<OrderModel[]>>(`${api}/orders/getall`, {
        headers: {
          "Authorization": "Bearer" + this.token()
        }
      }).subscribe(res => {
        this.orders.set(res.data!);
      });
    }
}
