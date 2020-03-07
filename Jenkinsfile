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
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform plan -out plan.tf
'''
        input 'Terraform Plan Review'
      }
    }

    stage('Apply') {
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform apply plan.tf
'''
      }
    }

  }
}