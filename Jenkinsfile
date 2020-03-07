pipeline {
  agent any
  stages {
    stage('Init') {
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

curl --output terraform-provider-avere --location --url $TERRAFORM_PROVIDER_AVERE_URL

chmod 755 terraform-provider-avere

terraform init
'''
      }
    }

    stage('Plan') {
      steps {
        sh '''az login --service-principal --tenant $AZURE_TENANT_ID --username $AZURE_CLIENT_ID --password $AZURE_CLIENT_SECRET

az account set --subscription $AZURE_SUBSCRIPTION_ID

cd $TERRAFORM_CONFIG_DIRECTORY

terraform plan
'''
      }
    }

    stage('Review') {
      steps {
        echo 'Review'
      }
    }

    stage('Apply') {
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform apply
'''
      }
    }

  }
}