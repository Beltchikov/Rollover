using IBApi;

namespace IbClient.Types
{
    public class OrderStateOrError
    {
        public OrderStateOrError(OrderState orderState, string errorMessage)
        {
            OrderState = orderState;
            ErrorMessage = errorMessage;
        }

        public OrderState OrderState { get; set; }
        public string ErrorMessage { get; set; }
    }
}
