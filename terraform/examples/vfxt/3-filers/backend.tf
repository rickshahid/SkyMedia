terraform {
  backend "azurerm" {
    storage_account_name = "mediastudio"
    container_name       = "terraform"
    key                  = "tfstate"
    use_msi              = true
  }
}
