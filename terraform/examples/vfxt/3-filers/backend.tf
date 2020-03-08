terraform {
  backend "azurerm" {
    resource_group_name  = "Azure.Media.Studio0-Storage"
    storage_account_name = "mediastudio"
    container_name       = "terraform"
    key                  = "tfstate"
    use_msi              = true
  }
}
