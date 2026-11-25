# Customer Management contribution

Adds CustomersController and Customers Razor views for Create, Read, Update and Delete, plus name/email/phone search and navigation.

## Run
1. Install .NET 8 SDK.
2. Open a terminal in `SmallBusinessInventory_CSharp`.
3. Run `dotnet restore` and `dotnet run`.
4. Open the local URL printed in the terminal and click **Customers**.

## Test checklist
- Create a customer with a name; check required name validation.
- Search by name, email or phone.
- View and edit the customer; verify changes persist after refresh.
- Delete a customer with confirmation; verify the record disappears.
- Check existing Products, Dashboard and Reports pages still work.

## Group workflow
Create a `feature/customer-management` branch, commit these changes, and submit a pull request for teammate review. Do not claim the code was tested until you run it on your own machine.
