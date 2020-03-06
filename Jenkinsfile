pipeline {
  agent any
  stages {
    stage('Init') {
      steps {
        sh '''cd terraform/examples/vfxt/3-filers/
curl --output terraform-provider-avere --url https://github.com/Azure/Avere/releases/download/tfprovider_v0.4.2/terraform-provider-avere
chmod 755 terraform-provider-avere
terraform init'''
      }
    }

    stage('Plan') {
      steps {
        sh '''terraform plan
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
        echo 'Apply'
      }
    }

  }
}