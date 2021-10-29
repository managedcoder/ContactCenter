using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace ComposerComponents
{
    public class RestateOrder : Dialog
    {

        [JsonConstructor]
        public RestateOrder([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "RestateOrder";

        [JsonProperty("Order")]
        public StringExpression Order { get; set; }

        [JsonProperty("resultProperty")]
        public StringExpression ResultProperty { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            /*
                This dialog shows how to use adaptive expressions to access and extract entities.  This is a case where adaptive expressions
                can get combersome so we've also included a custom action that simplifies things by dropping into C# to extract the entities.
                You can use the following pizza order to exercises the full LUIS model and extraction logic: "can i get a small margarita
                pizza please with bacon no onions and 2 large thin crust pizzas with half ham and the other half bacon and a coke and onion
                rings thanks"
            */
            string result = null;
            string deserializedOrder = Order.GetValue(dc.State);

            if (deserializedOrder != null)
            {
                dynamic order = JsonConvert.DeserializeObject(deserializedOrder);

                // PizzaType, Size, Quantity, Curst, ToppingsModifiers[]
                foreach (var pizza in order.FullPizzaWithModifiers)
                {
                    string pizzaType=null, size=null, quantity=null, crust=null, topping=null, toppings=null, scope=null, modifier=null;

                    // ${ if (contains(dialog.foreach.pizzaValue, 'PizzaType'), dialog.foreach.pizzaValue.PizzaType[0], 'pizza')}
                    if (pizza.PizzaType != null)
                        pizzaType = pizza.PizzaType[0];
                    
                    if (pizza.Size != null)
                        size = pizza.Size[0][0];

                    quantity = "1"; // defaults to 1 if not specified
                    if (pizza.Quantity != null)
                        quantity = pizza.Quantity[0];
                    
                    if (pizza.Crust != null)
                        crust = pizza.Crust[0][0] + " crust";

                    if (pizza.ToppingModifiers != null)
                    {
                        foreach (var toppingModifier in pizza.ToppingModifiers)
                        {
                            modifier = topping = scope = null;

                            if (toppingModifier.Modifier != null)
                            {
                                modifier = (toppingModifier.Modifier[0][0] == "Remove") ? " no " : "";
                            }

                            if (toppingModifier.Topping != null)
                            {
                                topping = toppingModifier.Topping[0][0];
                            }

                            if (toppingModifier.Scope != null)
                            {
                                scope = toppingModifier.Scope[0][0] + " ";
                            }

                            toppings = toppings + ((toppings != null) ? " and " + modifier + scope + topping : modifier + scope + topping);
                        }
                    }

                    result = $"{(result != null ? result + " " : result)} {quantity} {size} {crust} {pizzaType} {(toppings != null ? "with " + toppings : "")}  \n";
                }

                if (order.SideOrder != null)
                {
                    string sideOrder = null;
                    foreach (var product in order.SideOrder[0].SideProduct)
                    {
                        if (sideOrder == null)
                        {
                            sideOrder = "  \nand a side order of " + product;
                        }
                        else
                        {
                            sideOrder = sideOrder + " and " + product;
                        }
                    }

                    result = result + sideOrder;
                }
            }
            else
            {
                result = "No pizza detail specified";
            }

            if (this.ResultProperty != null)
            {
                dc.State.SetValue(this.ResultProperty.GetValue(dc.State), result);
            }

            return dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
        }
    }
}
