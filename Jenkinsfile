pipeline {
  agent any
  stages {
    stage('Init') {
      steps {
        sh '''cd terraform/examples/vfxt/3-filers/
curl --output terraform-provider-avere --location --url https://github.com/Azure/Avere/releases/download/tfprovider_v0.4.2/terraform-provider-avere
chmod 755 terraform-provider-avere
terraform init'''
      }
    }

    stage('Plan') {
      steps {
        sh '''az login --service-principal --tenant $TENANT_ID --username $CLIENT_ID --password $CLIENT_SECRET
cd terraform/examples/vfxt/3-filers/
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
        echo 'Apply'
      }
    }

  }
}