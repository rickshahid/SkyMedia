terraform {
  backend "azurerm" {
    storage_account_name = "mediastudio"
    container_name       = "terraform"
    use_msi              = true
  }
}
