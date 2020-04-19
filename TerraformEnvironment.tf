variable "ARM_SUBSCRIPTION_ID" {
 description = "Azure service resources subscription identifier"
 type = string
}

variable "ARM_USE_MSI" {
 description = "Enables Azure Managed Service Identity (MSI)"
 default = false
}
