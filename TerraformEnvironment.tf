variable "subscription_id" {
    description = "Azure service resources subscription identifier"
    type = string
}

variable "use_msi" {
    description = "Enables Azure Managed Service Identity (MSI)"
    default = false
}
