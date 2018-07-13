//
//  iOSScreenshot.m
//
//
//  Created by Ryan on 20/03/2013.
//
//


@implementation ShareViewController : UIViewController

-(void) shareMediaMethod: (const char *) path: (const char *) shareMessage
{
    NSString *imagePath = [NSString stringWithUTF8String:path];
    
    UIImage *image = [UIImage imageWithContentsOfFile:imagePath];
    NSString *message   = [NSString stringWithUTF8String:shareMessage];
    NSArray *postItems  = @[message,image];
    
    
    UIActivityViewController *activityVc = [[UIActivityViewController alloc]initWithActivityItems:postItems applicationActivities:nil];
    
    //Exclude Save to Gallery
    NSArray *excludeActivities = @[UIActivityTypeSaveToCameraRoll];
    activityVc.excludedActivityTypes = excludeActivities;
    
    if ( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
    {
        if ( [activityVc respondsToSelector:@selector(popoverPresentationController)] ) {
            
            UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityVc];
            
            [popup presentPopoverFromRect:CGRectMake(self.view.frame.size.width/2, self.view.frame.size.height/4, 0, 0)
                                   inView:[UIApplication sharedApplication].keyWindow.rootViewController.view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
        }
        else
            [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    }
    else
    {
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    }
    
    [activityVc release];
}



-(void) shareOnlyTextMethod: (const char *) shareMessage
{
    
    NSString *message   = [NSString stringWithUTF8String:shareMessage];
    NSArray *postItems  = @[message];
    
    UIActivityViewController *activityVc = [[UIActivityViewController alloc] initWithActivityItems:postItems applicationActivities:nil];
    
    //    // Check if IOS device is IOS8
    //    if ( [activityVc respondsToSelector:@selector(popoverPresentationController)] ) {
    //
    //        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityVc];
    //
    //        [popup presentPopoverFromRect:CGRectMake(self.view.frame.size.width/2, self.view.frame.size.height/4, 0, 0)
    //                               inView:[UIApplication sharedApplication].keyWindow.rootViewController.view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    //    }
    //    else
    //        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    
    
    
    if ( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
    {
        if ( [activityVc respondsToSelector:@selector(popoverPresentationController)] ) {
            
            UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityVc];
            
            [popup presentPopoverFromRect:CGRectMake(self.view.frame.size.width/2, self.view.frame.size.height/4, 0, 0)
                                   inView:[UIApplication sharedApplication].keyWindow.rootViewController.view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
        }
        else
            [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    }
    else
    {
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    }
    
    
    
    
    [activityVc release];
}


-(void)OpenFacebookURl: (const char *) fbPageID
{
    NSString *stringUrl = @"https://www.facebook.com/";
    NSString *facebookPageID = [NSString stringWithUTF8String: fbPageID];
    
    NSString *finalURL = [stringUrl stringByAppendingString:facebookPageID];
    NSURL *url = [NSURL URLWithString: finalURL];
    [[UIApplication sharedApplication] openURL:url];
}



@end



extern "C"{
    void MediaShareIos(const char * path, const char * message){
        ShareViewController *vc = [[ShareViewController alloc] init];
        [vc shareMediaMethod: path: message];
        [vc release];
    }
}
extern "C"{
    void TextShareIos(const char * message){
        ShareViewController *vc = [[ShareViewController alloc] init];
        [vc shareOnlyTextMethod: message];
        [vc release];
    }
}

extern "C"{
    void GotoFacebookPage(const char * pageID){
        ShareViewController *vc = [[ShareViewController alloc] init];
        [vc OpenFacebookURl: pageID];
        [vc release];
    }
}

extern "C"{
    void CreateAlertForRestore(const char* title, const char* body, const char * okbtn)
    {
        UIAlertView * alert = [[UIAlertView alloc] initWithTitle :[NSString stringWithUTF8String:title]
                                                          message:[NSString stringWithUTF8String:body] delegate: nil cancelButtonTitle:[NSString stringWithUTF8String:okbtn]
                                                otherButtonTitles: nil];
        [alert show];
    }
}

